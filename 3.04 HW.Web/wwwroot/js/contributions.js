
$(() => {

    $(".update-btn").on('click', function() {
        recalculateIndices();
        
    });

    function recalculateIndices() {
            let index = 0;
            $(".person-row").each(function () {
                const row = $(this);
                const inputs = row.find("input");
                inputs.each(function () {
                    const input = $(this);
                    const name = input.attr('name');
                    const indexOfDot = name.indexOf('.');
                    const attrName = name.substring(indexOfDot + 1);
                    input.attr('name', `contributor[${index}].${attrName}`);
                });
                index++;
            });
   
});