using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using GreenIotApi.Models;

namespace GreenIotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly InferenceSession _session;

        public AiController(InferenceSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session), "InferenceSession is null");
        }

        // POST api/ai/predict
        // Đầu vào là một mảng 2 chiều: ví dụ: [[20.5, 65, 10, 150, 5, 0]]
        [HttpPost("predict")]
        public IActionResult Predict([FromBody] float[][] input)
        {
            // Kiểm tra input
            if (input == null || input.Length == 0 || input[0].Length != 6)
            {
                return BadRequest("Input is null or the number of features is incorrect. Expected 6 features.");
            }

            // Tạo tensor từ dữ liệu đầu vào, shape: [1,6]
            var inputTensor = new DenseTensor<float>(input[0], new int[] { 1, 6 });
            Console.WriteLine("Input Tensor: " + string.Join(",", inputTensor.ToArray()));

            // Lấy tên input từ mô hình
            var inputName = _session.InputMetadata.Keys.First();
            var inputs = new NamedOnnxValue[] { NamedOnnxValue.CreateFromTensor(inputName, inputTensor) };
            Console.WriteLine("Inputs created successfully.");

            try
            {
                // Thực hiện dự đoán
                using (var results = _session.Run(inputs))
                {
                    // In ra tên các output để debug
                    var outputNames = results.Select(r => r.Name);
                    Console.WriteLine("Model output names: " + string.Join(", ", outputNames));

                    // Lấy output_label dưới dạng tensor của int (int64)
                    var labelTensor = results.FirstOrDefault(r => r.Name == "output_label")?.AsTensor<long>();
                    if (labelTensor == null || labelTensor.Length == 0)
                    {
                        return BadRequest("Model output_label is null or empty. Please check the model input and configuration.");
                    }
                    Console.WriteLine("Model output_label: " + string.Join(",", labelTensor.ToArray()));

                    // Lấy output_probability dưới dạng raw object, vì nó không phải tensor<float>
                    var outputProbabilityObject = results.FirstOrDefault(r => r.Name == "output_probability")?.Value;
                    if (outputProbabilityObject == null)
                    {
                        Console.WriteLine("Model output_probability is null.");
                    }
                    else
                    {
                        Console.WriteLine("Type of output_probability: " + outputProbabilityObject.GetType());
                        // Nếu cần, bạn có thể thử chuyển đổi hoặc in nội dung của output_probability
                    }

                    // Sử dụng output_label để xác định dự đoán
                    var prediction = labelTensor[0]; // Ví dụ: dự đoán là giá trị của output_label

                    return Ok(new { prediction, output_label = labelTensor.ToArray(), output_probability = outputProbabilityObject });
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error during model prediction: {ex.Message}");
            }
        }
    }
}
